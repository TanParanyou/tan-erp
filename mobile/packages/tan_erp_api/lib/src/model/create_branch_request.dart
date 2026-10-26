//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_branch_request.g.dart';

/// CreateBranchRequest
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [nameEn] 
/// * [taxBranchCode] 
/// * [addressTh] 
/// * [addressEn] 
/// * [phone] 
@BuiltValue()
abstract class CreateBranchRequest implements Built<CreateBranchRequest, CreateBranchRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'nameEn')
  String? get nameEn;

  @BuiltValueField(wireName: r'taxBranchCode')
  String? get taxBranchCode;

  @BuiltValueField(wireName: r'addressTh')
  String? get addressTh;

  @BuiltValueField(wireName: r'addressEn')
  String? get addressEn;

  @BuiltValueField(wireName: r'phone')
  String? get phone;

  CreateBranchRequest._();

  factory CreateBranchRequest([void updates(CreateBranchRequestBuilder b)]) = _$CreateBranchRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateBranchRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateBranchRequest> get serializer => _$CreateBranchRequestSerializer();
}

class _$CreateBranchRequestSerializer implements PrimitiveSerializer<CreateBranchRequest> {
  @override
  final Iterable<Type> types = const [CreateBranchRequest, _$CreateBranchRequest];

  @override
  final String wireName = r'CreateBranchRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateBranchRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.nameEn != null) {
      yield r'nameEn';
      yield serializers.serialize(
        object.nameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxBranchCode != null) {
      yield r'taxBranchCode';
      yield serializers.serialize(
        object.taxBranchCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.addressTh != null) {
      yield r'addressTh';
      yield serializers.serialize(
        object.addressTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.addressEn != null) {
      yield r'addressEn';
      yield serializers.serialize(
        object.addressEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.phone != null) {
      yield r'phone';
      yield serializers.serialize(
        object.phone,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateBranchRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateBranchRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'nameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nameEn = valueDes;
          break;
        case r'taxBranchCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxBranchCode = valueDes;
          break;
        case r'addressTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.addressTh = valueDes;
          break;
        case r'addressEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.addressEn = valueDes;
          break;
        case r'phone':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.phone = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateBranchRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateBranchRequestBuilder();
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


