//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'service_sla_summary_metrics_response.g.dart';

/// ServiceSlaSummaryMetricsResponse
///
/// Properties:
/// * [totalCount] 
/// * [openBacklogCount] 
/// * [waitingCustomerCount] 
/// * [slaWarningCount] 
/// * [slaBreachedCount] 
/// * [outOfWarrantyChargedTotal] 
@BuiltValue()
abstract class ServiceSlaSummaryMetricsResponse implements Built<ServiceSlaSummaryMetricsResponse, ServiceSlaSummaryMetricsResponseBuilder> {
  @BuiltValueField(wireName: r'totalCount')
  int? get totalCount;

  @BuiltValueField(wireName: r'openBacklogCount')
  int? get openBacklogCount;

  @BuiltValueField(wireName: r'waitingCustomerCount')
  int? get waitingCustomerCount;

  @BuiltValueField(wireName: r'slaWarningCount')
  int? get slaWarningCount;

  @BuiltValueField(wireName: r'slaBreachedCount')
  int? get slaBreachedCount;

  @BuiltValueField(wireName: r'outOfWarrantyChargedTotal')
  double? get outOfWarrantyChargedTotal;

  ServiceSlaSummaryMetricsResponse._();

  factory ServiceSlaSummaryMetricsResponse([void updates(ServiceSlaSummaryMetricsResponseBuilder b)]) = _$ServiceSlaSummaryMetricsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ServiceSlaSummaryMetricsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ServiceSlaSummaryMetricsResponse> get serializer => _$ServiceSlaSummaryMetricsResponseSerializer();
}

class _$ServiceSlaSummaryMetricsResponseSerializer implements PrimitiveSerializer<ServiceSlaSummaryMetricsResponse> {
  @override
  final Iterable<Type> types = const [ServiceSlaSummaryMetricsResponse, _$ServiceSlaSummaryMetricsResponse];

  @override
  final String wireName = r'ServiceSlaSummaryMetricsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ServiceSlaSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.totalCount != null) {
      yield r'totalCount';
      yield serializers.serialize(
        object.totalCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.openBacklogCount != null) {
      yield r'openBacklogCount';
      yield serializers.serialize(
        object.openBacklogCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.waitingCustomerCount != null) {
      yield r'waitingCustomerCount';
      yield serializers.serialize(
        object.waitingCustomerCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.slaWarningCount != null) {
      yield r'slaWarningCount';
      yield serializers.serialize(
        object.slaWarningCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.slaBreachedCount != null) {
      yield r'slaBreachedCount';
      yield serializers.serialize(
        object.slaBreachedCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.outOfWarrantyChargedTotal != null) {
      yield r'outOfWarrantyChargedTotal';
      yield serializers.serialize(
        object.outOfWarrantyChargedTotal,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ServiceSlaSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ServiceSlaSummaryMetricsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'totalCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.totalCount = valueDes;
          break;
        case r'openBacklogCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.openBacklogCount = valueDes;
          break;
        case r'waitingCustomerCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.waitingCustomerCount = valueDes;
          break;
        case r'slaWarningCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.slaWarningCount = valueDes;
          break;
        case r'slaBreachedCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.slaBreachedCount = valueDes;
          break;
        case r'outOfWarrantyChargedTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.outOfWarrantyChargedTotal = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ServiceSlaSummaryMetricsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ServiceSlaSummaryMetricsResponseBuilder();
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


