//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/ar_aging_customer_bucket_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'ar_aging_report_response.g.dart';

/// ArAgingReportResponse
///
/// Properties:
/// * [asOfDate] 
/// * [totalCurrent] 
/// * [totalBucket1To30] 
/// * [totalBucket31To60] 
/// * [totalBucket61To90] 
/// * [totalBucketOver90] 
/// * [grandTotalOutstanding] 
/// * [customers] 
@BuiltValue()
abstract class ArAgingReportResponse implements Built<ArAgingReportResponse, ArAgingReportResponseBuilder> {
  @BuiltValueField(wireName: r'asOfDate')
  Date? get asOfDate;

  @BuiltValueField(wireName: r'totalCurrent')
  double? get totalCurrent;

  @BuiltValueField(wireName: r'totalBucket1To30')
  double? get totalBucket1To30;

  @BuiltValueField(wireName: r'totalBucket31To60')
  double? get totalBucket31To60;

  @BuiltValueField(wireName: r'totalBucket61To90')
  double? get totalBucket61To90;

  @BuiltValueField(wireName: r'totalBucketOver90')
  double? get totalBucketOver90;

  @BuiltValueField(wireName: r'grandTotalOutstanding')
  double? get grandTotalOutstanding;

  @BuiltValueField(wireName: r'customers')
  BuiltList<ArAgingCustomerBucketResponse>? get customers;

  ArAgingReportResponse._();

  factory ArAgingReportResponse([void updates(ArAgingReportResponseBuilder b)]) = _$ArAgingReportResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ArAgingReportResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ArAgingReportResponse> get serializer => _$ArAgingReportResponseSerializer();
}

class _$ArAgingReportResponseSerializer implements PrimitiveSerializer<ArAgingReportResponse> {
  @override
  final Iterable<Type> types = const [ArAgingReportResponse, _$ArAgingReportResponse];

  @override
  final String wireName = r'ArAgingReportResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ArAgingReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.asOfDate != null) {
      yield r'asOfDate';
      yield serializers.serialize(
        object.asOfDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.totalCurrent != null) {
      yield r'totalCurrent';
      yield serializers.serialize(
        object.totalCurrent,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalBucket1To30 != null) {
      yield r'totalBucket1To30';
      yield serializers.serialize(
        object.totalBucket1To30,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalBucket31To60 != null) {
      yield r'totalBucket31To60';
      yield serializers.serialize(
        object.totalBucket31To60,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalBucket61To90 != null) {
      yield r'totalBucket61To90';
      yield serializers.serialize(
        object.totalBucket61To90,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalBucketOver90 != null) {
      yield r'totalBucketOver90';
      yield serializers.serialize(
        object.totalBucketOver90,
        specifiedType: const FullType(double),
      );
    }
    if (object.grandTotalOutstanding != null) {
      yield r'grandTotalOutstanding';
      yield serializers.serialize(
        object.grandTotalOutstanding,
        specifiedType: const FullType(double),
      );
    }
    if (object.customers != null) {
      yield r'customers';
      yield serializers.serialize(
        object.customers,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ArAgingCustomerBucketResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ArAgingReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ArAgingReportResponseBuilder result,
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
        case r'totalCurrent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalCurrent = valueDes;
          break;
        case r'totalBucket1To30':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalBucket1To30 = valueDes;
          break;
        case r'totalBucket31To60':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalBucket31To60 = valueDes;
          break;
        case r'totalBucket61To90':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalBucket61To90 = valueDes;
          break;
        case r'totalBucketOver90':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalBucketOver90 = valueDes;
          break;
        case r'grandTotalOutstanding':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.grandTotalOutstanding = valueDes;
          break;
        case r'customers':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ArAgingCustomerBucketResponse)]),
          ) as BuiltList<ArAgingCustomerBucketResponse>?;
          if (valueDes == null) continue;
          result.customers.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ArAgingReportResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ArAgingReportResponseBuilder();
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


