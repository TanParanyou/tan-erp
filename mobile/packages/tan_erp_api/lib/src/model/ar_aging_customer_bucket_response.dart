//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'ar_aging_customer_bucket_response.g.dart';

/// ArAgingCustomerBucketResponse
///
/// Properties:
/// * [customerId] 
/// * [customerCode] 
/// * [customerName] 
/// * [current] 
/// * [bucket1To30] 
/// * [bucket31To60] 
/// * [bucket61To90] 
/// * [bucketOver90] 
/// * [totalOutstanding] 
@BuiltValue()
abstract class ArAgingCustomerBucketResponse implements Built<ArAgingCustomerBucketResponse, ArAgingCustomerBucketResponseBuilder> {
  @BuiltValueField(wireName: r'customerId')
  String? get customerId;

  @BuiltValueField(wireName: r'customerCode')
  String? get customerCode;

  @BuiltValueField(wireName: r'customerName')
  String? get customerName;

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

  ArAgingCustomerBucketResponse._();

  factory ArAgingCustomerBucketResponse([void updates(ArAgingCustomerBucketResponseBuilder b)]) = _$ArAgingCustomerBucketResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ArAgingCustomerBucketResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ArAgingCustomerBucketResponse> get serializer => _$ArAgingCustomerBucketResponseSerializer();
}

class _$ArAgingCustomerBucketResponseSerializer implements PrimitiveSerializer<ArAgingCustomerBucketResponse> {
  @override
  final Iterable<Type> types = const [ArAgingCustomerBucketResponse, _$ArAgingCustomerBucketResponse];

  @override
  final String wireName = r'ArAgingCustomerBucketResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ArAgingCustomerBucketResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.customerId != null) {
      yield r'customerId';
      yield serializers.serialize(
        object.customerId,
        specifiedType: const FullType(String),
      );
    }
    if (object.customerCode != null) {
      yield r'customerCode';
      yield serializers.serialize(
        object.customerCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.customerName != null) {
      yield r'customerName';
      yield serializers.serialize(
        object.customerName,
        specifiedType: const FullType.nullable(String),
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
    ArAgingCustomerBucketResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ArAgingCustomerBucketResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'customerId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerId = valueDes;
          break;
        case r'customerCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerCode = valueDes;
          break;
        case r'customerName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerName = valueDes;
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
  ArAgingCustomerBucketResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ArAgingCustomerBucketResponseBuilder();
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


