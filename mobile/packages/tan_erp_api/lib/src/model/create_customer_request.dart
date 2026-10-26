//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/create_primary_contact_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_customer_request.g.dart';

/// CreateCustomerRequest
///
/// Properties:
/// * [customerType] 
/// * [displayNameTh] 
/// * [displayNameEn] 
/// * [preferredLocale] 
/// * [primaryContact] 
/// * [leadSource] 
/// * [leadSourceNote] 
/// * [imageFileId] 
/// * [fileUploadIntentId] 
@BuiltValue()
abstract class CreateCustomerRequest implements Built<CreateCustomerRequest, CreateCustomerRequestBuilder> {
  @BuiltValueField(wireName: r'customerType')
  String? get customerType;

  @BuiltValueField(wireName: r'displayNameTh')
  String? get displayNameTh;

  @BuiltValueField(wireName: r'displayNameEn')
  String? get displayNameEn;

  @BuiltValueField(wireName: r'preferredLocale')
  String? get preferredLocale;

  @BuiltValueField(wireName: r'primaryContact')
  CreatePrimaryContactRequest? get primaryContact;

  @BuiltValueField(wireName: r'leadSource')
  String? get leadSource;

  @BuiltValueField(wireName: r'leadSourceNote')
  String? get leadSourceNote;

  @BuiltValueField(wireName: r'imageFileId')
  String? get imageFileId;

  @BuiltValueField(wireName: r'fileUploadIntentId')
  String? get fileUploadIntentId;

  CreateCustomerRequest._();

  factory CreateCustomerRequest([void updates(CreateCustomerRequestBuilder b)]) = _$CreateCustomerRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateCustomerRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateCustomerRequest> get serializer => _$CreateCustomerRequestSerializer();
}

class _$CreateCustomerRequestSerializer implements PrimitiveSerializer<CreateCustomerRequest> {
  @override
  final Iterable<Type> types = const [CreateCustomerRequest, _$CreateCustomerRequest];

  @override
  final String wireName = r'CreateCustomerRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateCustomerRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.customerType != null) {
      yield r'customerType';
      yield serializers.serialize(
        object.customerType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayNameTh != null) {
      yield r'displayNameTh';
      yield serializers.serialize(
        object.displayNameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayNameEn != null) {
      yield r'displayNameEn';
      yield serializers.serialize(
        object.displayNameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.preferredLocale != null) {
      yield r'preferredLocale';
      yield serializers.serialize(
        object.preferredLocale,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.primaryContact != null) {
      yield r'primaryContact';
      yield serializers.serialize(
        object.primaryContact,
        specifiedType: const FullType(CreatePrimaryContactRequest),
      );
    }
    if (object.leadSource != null) {
      yield r'leadSource';
      yield serializers.serialize(
        object.leadSource,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.leadSourceNote != null) {
      yield r'leadSourceNote';
      yield serializers.serialize(
        object.leadSourceNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.imageFileId != null) {
      yield r'imageFileId';
      yield serializers.serialize(
        object.imageFileId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.fileUploadIntentId != null) {
      yield r'fileUploadIntentId';
      yield serializers.serialize(
        object.fileUploadIntentId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateCustomerRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateCustomerRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'customerType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerType = valueDes;
          break;
        case r'displayNameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayNameTh = valueDes;
          break;
        case r'displayNameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayNameEn = valueDes;
          break;
        case r'preferredLocale':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.preferredLocale = valueDes;
          break;
        case r'primaryContact':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CreatePrimaryContactRequest),
          ) as CreatePrimaryContactRequest?;
          if (valueDes == null) continue;
          result.primaryContact.replace(valueDes);
          break;
        case r'leadSource':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.leadSource = valueDes;
          break;
        case r'leadSourceNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.leadSourceNote = valueDes;
          break;
        case r'imageFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.imageFileId = valueDes;
          break;
        case r'fileUploadIntentId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fileUploadIntentId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateCustomerRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateCustomerRequestBuilder();
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


