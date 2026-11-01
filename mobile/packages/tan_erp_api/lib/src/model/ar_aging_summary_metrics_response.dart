//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'ar_aging_summary_metrics_response.g.dart';

/// ArAgingSummaryMetricsResponse
///
/// Properties:
/// * [asOfDate] 
/// * [invoiceCount] 
/// * [current] 
/// * [bucket1To30] 
/// * [bucket31To60] 
/// * [bucket61To90] 
/// * [bucketOver90] 
/// * [totalOutstanding] 
@BuiltValue()
abstract class ArAgingSummaryMetricsResponse implements Built<ArAgingSummaryMetricsResponse, ArAgingSummaryMetricsResponseBuilder> {
  @BuiltValueField(wireName: r'asOfDate')
  Date? get asOfDate;

  @BuiltValueField(wireName: r'invoiceCount')
  int? get invoiceCount;

  @BuiltValueField(wireName: r'current')
  double? get current;

  @BuiltValueField(wireName: r'bucket1To30')
  double? get bucket1To30;

  @BuiltValueField(wireName: r'bucket31To60')
  double? get bucket31To60;

  @BuiltValueField(wireName: r'bucket61To90')
  double? get bucket61To90;

  @BuiltValueField(wireName: r'bucketOver90')
  double? get bucketOver90;

  @BuiltValueField(wireName: r'totalOutstanding')
  double? get totalOutstanding;

  ArAgingSummaryMetricsResponse._();

  factory ArAgingSummaryMetricsResponse([void updates(ArAgingSummaryMetricsResponseBuilder b)]) = _$ArAgingSummaryMetricsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ArAgingSummaryMetricsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ArAgingSummaryMetricsResponse> get serializer => _$ArAgingSummaryMetricsResponseSerializer();
}

class _$ArAgingSummaryMetricsResponseSerializer implements PrimitiveSerializer<ArAgingSummaryMetricsResponse> {
  @override
  final Iterable<Type> types = const [ArAgingSummaryMetricsResponse, _$ArAgingSummaryMetricsResponse];

  @override
  final String wireName = r'ArAgingSummaryMetricsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ArAgingSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.asOfDate != null) {
      yield r'asOfDate';
      yield serializers.serialize(
        object.asOfDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.invoiceCount != null) {
      yield r'invoiceCount';
      yield serializers.serialize(
        object.invoiceCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.current != null) {
      yield r'current';
      yield serializers.serialize(
        object.current,
        specifiedType: const FullType(double),
      );
    }
    if (object.bucket1To30 != null) {
      yield r'bucket1To30';
      yield serializers.serialize(
        object.bucket1To30,
        specifiedType: const FullType(double),
      );
    }
    if (object.bucket31To60 != null) {
      yield r'bucket31To60';
      yield serializers.serialize(
        object.bucket31To60,
        specifiedType: const FullType(double),
      );
    }
    if (object.bucket61To90 != null) {
      yield r'bucket61To90';
      yield serializers.serialize(
        object.bucket61To90,
        specifiedType: const FullType(double),
      );
    }
    if (object.bucketOver90 != null) {
      yield r'bucketOver90';
      yield serializers.serialize(
        object.bucketOver90,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalOutstanding != null) {
      yield r'totalOutstanding';
      yield serializers.serialize(
        object.totalOutstanding,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ArAgingSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ArAgingSummaryMetricsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'asOfDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.asOfDate = valueDes;
          break;
        case r'invoiceCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.invoiceCount = valueDes;
          break;
        case r'current':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.current = valueDes;
          break;
        case r'bucket1To30':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.bucket1To30 = valueDes;
          break;
        case r'bucket31To60':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.bucket31To60 = valueDes;
          break;
        case r'bucket61To90':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.bucket61To90 = valueDes;
          break;
        case r'bucketOver90':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.bucketOver90 = valueDes;
          break;
        case r'totalOutstanding':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalOutstanding = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ArAgingSummaryMetricsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ArAgingSummaryMetricsResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


